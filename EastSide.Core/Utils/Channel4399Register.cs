using Codexus.Development.SDK.Entities;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Codexus.Cipher.Entities.Pc4399;
using Codexus.Cipher.Utils;
using Codexus.Cipher.Utils.Http;
using Serilog;

namespace EastSide.Core.Utils;

public class Channel4399Register : IDisposable
{
  private readonly HttpClient _httpClient;

  public Channel4399Register()
  {
    _httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true })
    {
      BaseAddress = new Uri("https://ptlogin.4399.com")
    };
  }

  public void Dispose()
  {
    _httpClient?.Dispose();
    GC.SuppressFinalize(this);
  }

  public async Task<Entity4399Account> RegisterAsync(
    Func<string, Task<string>> inputCaptchaAsync,
    Func<IdCard> idCardFunc)
  {
    int maxRetries = 5;
    int retryCount = 0;

    while (retryCount < maxRetries)
    {
      try
      {
        string account = "O2_" + RandomUtil.GetRandomString(6);
        string password = RandomUtil.GetRandomString(8);
        string captchaId = Guid.NewGuid().ToString("N");
        string captchaUrl = "https://ptlogin.4399.com/ptlogin/captcha.do?captchaId=" + captchaId;

        string captcha = await CaptchaRecognitionService.RecognizeFromUrlAsync(captchaUrl);

        Log.Information($"自动识别验证码: {captcha}");

        IdCard idCard = idCardFunc();
        HttpResponseMessage response =
          await _httpClient.GetAsync(BuildRegisterUrl(captchaId, captcha, account, password, idCard.IdNumber,
            idCard.Name));
        if (!response.IsSuccessStatusCode)
          throw new Exception("Status Code:" + response.StatusCode);

        string content = await response.Content.ReadAsStringAsync();

        if (content.Contains("验证码错误"))
          throw new Exception("验证码错误");
        if (content.Contains("用户名已被注册"))
          throw new Exception("用户名已被注册");
        if (!content.Contains("请一定记住您注册的用户名和密码"))
          throw new Exception("注册失败，请重试");

        await PerformRealNameVerification(account, password);

        Entity4399Account entity4399Account = new Entity4399Account
        {
          Account = account,
          Password = password
        };

        return entity4399Account;
      }
      catch (Exception ex)
      {
        retryCount++;
        Log.Information($"注册失败，正在进行第 {retryCount} 次重试: {ex.Message}");

        if (retryCount >= maxRetries)
        {
          throw;
        }

        await Task.Delay(2000);
      }
    }

    throw new Exception("达到最大重试次数，注册失败");
  }

  private static string BuildRegisterUrl(
    string captchaId,
    string captcha,
    string account,
    string password,
    string idCard,
    string name)
  {
    return "/ptlogin/register.do?" + new ParameterBuilder().Append("postLoginHandler", "default")
      .Append("displayMode", "popup").Append("appId", "www_home").Append("gameId", "").Append("cid", "")
      .Append("externalLogin", "qq").Append("aid", "").Append("ref", "").Append("css", "").Append("redirectUrl", "")
      .Append("regMode", "reg_normal").Append("sessionId", captchaId).Append("regIdcard", "true")
      .Append("noEmail", "false").Append("crossDomainIFrame", "").Append("crossDomainUrl", "")
      .Append("mainDivId", "popup_reg_div").Append("showRegInfo", "true").Append("includeFcmInfo", "false")
      .Append("expandFcmInput", "true").Append("fcmFakeValidate", "true").Append("userNameLabel", "4399用户名")
      .Append("username", account).Append(nameof(password), password).Append("realname", name).Append("idcard", idCard)
      .Append("email", RandomUtil.GetRandomString(10, "0123456789") + "@qq.com").Append("reg_eula_agree", "on")
      .Append("inputCaptcha", captcha).FormUrlEncode();
  }

  private async Task PerformRealNameVerification(string username, string password)
  {
    Log.Information($"开始为账号 {username} 进行本地实名认证...");

    try
    {
      var resourceName = "EastSide.Assets.sfz.txt";

      var assembly = AppDomain.CurrentDomain.GetAssemblies()
          .FirstOrDefault(a => a.GetName().Name == "EastSide")
          ?? System.Reflection.Assembly.GetExecutingAssembly();

      using var stream = assembly.GetManifestResourceStream(resourceName);
      if (stream == null)
      {
        Log.Warning($"未找到嵌入的身份证信息文件: {resourceName}");
        return;
      }

      var idCards = new List<(string Name, string IdCard)>();
      using var reader = new StreamReader(stream);

      while (!reader.EndOfStream)
      {
        var line = await reader.ReadLineAsync();
        if (line == null) continue;

        var cleanLine = line.Trim();
        if (string.IsNullOrEmpty(cleanLine) || cleanLine.StartsWith("#"))
          continue;

        var parts = System.Text.RegularExpressions.Regex.Split(cleanLine, @"[\t\s]+").Take(2).ToArray();
        if (parts.Length >= 2)
        {
          var name = parts[0].Trim();
          var idCard = parts[1].Trim();
          if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(idCard))
          {
            idCards.Add((name, idCard));
          }
        }
      }

      if (idCards.Count == 0)
      {
        Log.Warning("身份证信息文件中没有有效数据");
        return;
      }

      Log.Information($"已加载 {idCards.Count} 条身份证信息");

      var realNameTool = new RealNameTool();
      await realNameTool.RunAsync(idCards, username, password);

      Log.Information($"账号 {username} 实名认证流程已完成");
    }
    catch (Exception ex)
    {
      Log.Error(ex, $"本地实名认证时发生错误");
    }
  }

  public static string GenerateRandomIdCard()
  {
    string idCard = $"110108{GetRandomDate("19700101", "20041231")}{RandomUtil.GetRandomString(3, "0123456789")}";
    return idCard + GetIdCardLastCode(idCard);
  }

  public static string GenerateChineseName()
  {
    ReadOnlySpan<char> randomString = (ReadOnlySpan<char>)RandomUtil.GetRandomString(1,
      "李王张刘陈杨赵黄周吴徐孙胡朱高林何郭马罗梁宋郑谢韩唐冯于董萧程曹袁邓许傅沈曾彭吕苏卢蒋蔡贾丁魏薛叶阎余潘杜戴夏钟汪田任姜范方石姚谭廖邹熊金陆郝孔白崔康毛邱秦江史顾侯邵孟龙万段漕钱汤尹黎易常武乔贺赖龚文");
    char chineseCharacter1 = GenerateChineseCharacter();
    ReadOnlySpan<char> readOnlySpan1 = new ReadOnlySpan<char>(ref chineseCharacter1);
    char chineseCharacter2 = GenerateChineseCharacter();
    ReadOnlySpan<char> readOnlySpan2 = new ReadOnlySpan<char>(ref chineseCharacter2);
    return randomString.ToString() + readOnlySpan1.ToString() + readOnlySpan2.ToString();
  }

  private static char GenerateChineseCharacter() => (char)Random.Shared.Next(19968, 40870);

  private static string GetRandomDate(string startDate, string endDate)
  {
    DateTime exact = DateTime.ParseExact(startDate, "yyyyMMdd", CultureInfo.InvariantCulture);
    int days = (DateTime.ParseExact(endDate, "yyyyMMdd", CultureInfo.InvariantCulture) - exact).Days;
    return exact.AddDays(Random.Shared.Next(days)).ToString("yyyyMMdd");
  }

  private static string GetIdCardLastCode(string idCard)
  {
    int[] factors =
    {
      7,
      9,
      10,
      5,
      8,
      4,
      2,
      1,
      6,
      3,
      7,
      9,
      10,
      5,
      8,
      4,
      2
    };
    return new string[]
    {
      "1",
      "0",
      "X",
      "9",
      "8",
      "7",
      "6",
      "5",
      "4",
      "3",
      "2"
    }[idCard.Take(17).Select((Func<char, int, int>)((c, i) => ((int)c - 48) * factors[i])).Sum() % 11];
  }
}
