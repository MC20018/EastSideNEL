using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EastSide.Core.Utils
{
    public class RealNameTool
    {
        private const string BaseUrl = "http://ptlogin.4399.com";
        private const string HttpsBaseUrl = "https://ptlogin.4399.com";

        private const string VerifyUrl = BaseUrl + "/ptlogin/verify.do";
        private const string CaptchaUrl = BaseUrl + "/ptlogin/captcha.do";
        private const string LoginUrl = BaseUrl + "/ptlogin/login.do";
        private const string CheckKidUrl = BaseUrl + "/ptlogin/checkKidLoginUserCookie.do";
        private const string SetRealnameUrl = HttpsBaseUrl + "/ptlogin/setIdcardAndRealname.do";

        private const string GameUrl = "http://cdn.h5wan.4399sj.com/microterminal-h5-frame" +
            "?game_id=500352&rand_time=&nick=null&onLineStart=false" +
            "&show=1&isCrossDomain=1" +
            "&retUrl=http%253A%252F%252Fptlogin.4399.com%252Fresource%252Fucenter.html";

        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36";

        private static readonly Dictionary<int, string> ErrorCodes = new Dictionary<int, string>
        {
            { 3, "审核成功" },
            { 3002, "您已经验证过了" },
            { 3006, "您还未登录" },
            { 3007, "填写有错误" },
            { 309, "身份证姓名不匹配，请重新认证" },
            { 313, "监护人不得为未成年人" },
            { 315, "之前提交的信息正在审核" },
            { 375, "业务校验失败" },
            { 379, "身份证号码异常" },
            { 394, "身份证实名账号数量超过限制" },
            { 402, "身份证实名认证账号数量超过时段限制" }
        };

        private static readonly int[] SuccessCodes = { 3, 3002 };

        private HttpClient _httpClient;
        private List<(string Name, string IdCard)> _idCards;
        private Random _random = new Random();

        public RealNameTool()
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            
            _idCards = new List<(string, string)>();
        }

        public bool LoadSfzFromEmbeddedResource()
        {
            try
            {
                var resourceName = "EastSide.Assets.sfz.txt";

                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "EastSide")
                    ?? System.Reflection.Assembly.GetExecutingAssembly();
                
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    Console.WriteLine($"错误: 未找到内嵌资源 {resourceName}");
                    return false;
                }

                using var reader = new StreamReader(stream);
                _idCards.Clear();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    
                    var cleanLine = line.Trim();
                    if (string.IsNullOrEmpty(cleanLine) || cleanLine.StartsWith("#"))
                        continue;

                    var parts = Regex.Split(cleanLine, @"[:\t\s]+").Take(2).ToArray();
                    if (parts.Length >= 2)
                    {
                        var name = parts[0].Trim();
                        var idCard = parts[1].Trim();
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(idCard))
                        {
                            _idCards.Add((name, idCard));
                        }
                    }
                }

                if (_idCards.Count == 0)
                {
                    Console.WriteLine($"错误: 内嵌资源中没有有效的身份证信息。");
                    return false;
                }

                Console.WriteLine($"已加载 {_idCards.Count} 条身份证信息。");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"读取内嵌资源失败: {e.Message}");
                return false;
            }
        }
        
        public async Task<(bool NeedCaptcha, string? CaptchaId)> CheckNeedCaptchaAsync(string username)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "username", username },
                    { "appId", "" },
                    { "t", "" },
                    { "inputWidth", "iptw2" },
                    { "v", "1" }
                };

                var queryString = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value ?? "")}"));
                var url = $"{VerifyUrl}?{queryString}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var text = await response.Content.ReadAsStringAsync();

                var match = Regex.Match(text, @"captchaId[""'\s:=]+[""']?(\w+)");
                if (match.Success)
                {
                    return (true, match.Groups[1].Value);
                }

                try
                {
                    using var doc = JsonDocument.Parse(text);
                    if (doc.RootElement.TryGetProperty("captchaId", out var captchaIdElement))
                    {
                        return (true, captchaIdElement.GetString());
                    }
                }
                catch
                {
                }

                return (false, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"检查验证码状态失败: {e.Message}");
                return (false, null);
            }
        }

        public async Task<string> GetCaptchaAsync(string captchaId)
        {
            try
            {
                var url = $"{CaptchaUrl}?captchaId={captchaId}";
                
                string recognizedCaptcha = await CaptchaRecognitionService.RecognizeFromUrlAsync(url);
                
                Console.WriteLine($"验证码识别结果: {recognizedCaptcha ?? "识别失败"}");
                
                if (!string.IsNullOrEmpty(recognizedCaptcha))
                {
                    Console.WriteLine($"自动识别验证码: {recognizedCaptcha}");
                    return recognizedCaptcha;
                }
                
                Console.WriteLine("自动识别验证码失败，使用手动输入模式");
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var captchaPath = Path.Combine(Directory.GetCurrentDirectory(), "captcha.png");
                await File.WriteAllBytesAsync(captchaPath, await response.Content.ReadAsByteArrayAsync());

                try
                {
                    OpenImage(captchaPath);
                }
                catch
                {
                    Console.WriteLine($"验证码已保存到: {captchaPath}");
                }

                Console.Write("请输入验证码: ");
                return Console.ReadLine()?.Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine($"处理验证码时出错: {e.Message}");
                return null;
            }
        }

        private void OpenImage(string imagePath)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = imagePath,
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsLinux())
                {
                    System.Diagnostics.Process.Start("xdg-open", imagePath);
                }
                else if (OperatingSystem.IsMacOS())
                {
                    System.Diagnostics.Process.Start("open", imagePath);
                }
            }
            catch
            {
                Console.WriteLine($"验证码已保存到: {imagePath}");
            }
        }

        public async Task<(bool Success, string Message)> LoginAsync(string username, string password, string captcha = "")
        {
            try
            {
                Console.WriteLine("开始登录流程...");

                var (needCaptcha, captchaId) = await CheckNeedCaptchaAsync(username);

                if (needCaptcha && !string.IsNullOrEmpty(captchaId) && string.IsNullOrEmpty(captcha))
                {
                    captcha = await GetCaptchaAsync(captchaId);
                    if (string.IsNullOrEmpty(captcha))
                    {
                        return (false, "验证码获取失败");
                    }
                }

                var data = new Dictionary<string, string>
                {
                    { "username", username },
                    { "password", password },
                    { "inputCaptcha", captcha },
                    { "sec1", "" },
                    { "appId", "" },
                    { "gameId", "" },
                    { "bizId", "" },
                    { "redirectUrl", "" },
                    { "sessionId", "" },
                    { "postLoginHandler", "default" },
                    { "externalLogin", "qq" }
                };

                var content = new FormUrlEncodedContent(data);
                var response = await _httpClient.PostAsync($"{LoginUrl}?v=1", content);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return (false, $"登录失败，HTTP 状态码: {(int)response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                
                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("code", out var codeElement) && codeElement.GetInt32() != 0)
                    {
                        var messageElement = doc.RootElement.GetProperty("message");
                        return (false, $"登录失败: {messageElement.GetString()}");
                    }
                }
                catch
                {
                }

                if (responseBody.Contains("USESSIONID") || responseBody.Contains("Pauth") || 
                    !responseBody.Contains("error") || !responseBody.Contains("失败"))
                {
                    return (true, "登录成功");
                }

                return (false, "登录失败: 缺失认证 Cookie。");
            }
            catch (TaskCanceledException)
            {
                return (false, "登录超时");
            }
            catch (Exception e)
            {
                return (false, $"登录异常: {e.Message}");
            }
        }

        public async Task<(bool NeedValidate, string Error)> CheckRealnameStatusAsync()
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "appId", "" },
                    { "gameUrl", GameUrl }
                };

                var queryString = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value ?? "")}"));
                var url = $"{CheckKidUrl}?{queryString}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var text = await response.Content.ReadAsStringAsync();

                try
                {
                    var jsonMatch = Regex.Match(text, @"\{.*\}", RegexOptions.Singleline);
                    if (jsonMatch.Success)
                    {
                        using var doc = JsonDocument.Parse(jsonMatch.Value);
                        if (doc.RootElement.TryGetProperty("needValidate", out var needValidateElement))
                        {
                            return (needValidateElement.GetBoolean(), null);
                        }

                        if (doc.RootElement.TryGetProperty("code", out var codeElement) && codeElement.GetInt32() == 3002)
                        {
                            return (false, null);
                        }
                    }
                }
                catch
                {
                }

                if (text.Contains("needValidate"))
                {
                    if (text.ToLower().Contains("true"))
                    {
                        return (true, null);
                    }
                    else if (text.ToLower().Contains("false"))
                    {
                        return (false, null);
                    }
                }

                return (true, null);
            }
            catch (Exception e)
            {
                return (true, $"检查实名状态失败: {e.Message}");
            }
        }

        public async Task<(bool Success, int Code, string Message)> DoRealnameAsync(string realname, string idcard)
        {
            try
            {
                var data = new Dictionary<string, string>
                {
                    { "realname", realname },
                    { "idcard", idcard },
                    { "appid", "" },
                    { "isReg", "" }
                };

                var content = new FormUrlEncodedContent(data);
                var response = await _httpClient.PostAsync(SetRealnameUrl, content);

                response.EnsureSuccessStatusCode();

                var text = (await response.Content.ReadAsStringAsync()).Trim();

                try
                {
                    var jsonMatch = Regex.Match(text, @"\{.*\}", RegexOptions.Singleline);
                    JsonDocument resultDoc;

                    if (jsonMatch.Success)
                    {
                        resultDoc = JsonDocument.Parse(jsonMatch.Value);
                    }
                    else
                    {
                        resultDoc = JsonDocument.Parse(text);
                    }

                    if (resultDoc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        var codeElement = resultDoc.RootElement.GetProperty("code");
                        int code = codeElement.GetInt32();

                        string message = "";
                        if (resultDoc.RootElement.TryGetProperty("message", out var messageElement))
                        {
                            message = messageElement.GetString();
                        }
                        else if (resultDoc.RootElement.TryGetProperty("msg", out var msgElement))
                        {
                            message = msgElement.GetString();
                        }

                        if (SuccessCodes.Contains(code))
                        {
                            return (true, code, ErrorCodes.ContainsKey(code) ? ErrorCodes[code] : "成功");
                        }
                        else
                        {
                            string errorMsg = ErrorCodes.ContainsKey(code) ? ErrorCodes[code] : message ?? "未知错误";
                            return (false, code, errorMsg);
                        }
                    }
                    else if (resultDoc.RootElement.ValueKind == JsonValueKind.Number)
                    {
                        int code = resultDoc.RootElement.GetInt32();
                        if (SuccessCodes.Contains(code))
                        {
                            return (true, code, ErrorCodes.ContainsKey(code) ? ErrorCodes[code] : "成功");
                        }
                        else
                        {
                            return (false, code, ErrorCodes.ContainsKey(code) ? ErrorCodes[code] : "未知错误");
                        }
                    }
                    else
                    {
                        int code = -1;
                        string message = resultDoc.RootElement.ToString();
                        return (false, code, message);
                    }
                }
                catch (JsonException)
                {
                    var codeMatch = Regex.Match(text, @"""?code""?\s*[:\s=]\s*(\d+)");
                    if (codeMatch.Success)
                    {
                        int code = int.Parse(codeMatch.Groups[1].Value);
                        if (SuccessCodes.Contains(code))
                        {
                            return (true, code, ErrorCodes.ContainsKey(code) ? ErrorCodes[code] : "成功");
                        }
                        else
                        {
                            return (false, code, ErrorCodes.ContainsKey(code) ? ErrorCodes[code] : "未知错误");
                        }
                    }

                    if (int.TryParse(text, out int parsedCode))
                    {
                        if (SuccessCodes.Contains(parsedCode))
                        {
                            return (true, parsedCode, ErrorCodes.ContainsKey(parsedCode) ? ErrorCodes[parsedCode] : "成功");
                        }
                        else
                        {
                            return (false, parsedCode, ErrorCodes.ContainsKey(parsedCode) ? ErrorCodes[parsedCode] : "未知错误");
                        }
                    }

                    return (false, -1, $"解析响应失败: {text.Substring(0, Math.Min(100, text.Length))}");
                }
            }
            catch (TaskCanceledException)
            {
                return (false, -1, "请求超时");
            }
            catch (Exception e)
            {
                return (false, -1, $"认证异常: {e.Message}");
            }
        }

        public async Task RunAsync(List<(string Name, string IdCard)> idCards, string username, string password)
        {
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("4399 实名认证工具 (C# 版)");
            Console.WriteLine("原作者: freecookie.studio");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine();

            _idCards = idCards;

            if (_idCards.Count == 0)
            {
                Console.WriteLine("错误: 没有有效的身份证信息。");
                return;
            }

            Console.WriteLine();

            var (success, msg) = await LoginAsync(username, password);
            if (!success)
            {
                Console.WriteLine(msg);
                return;
            }

            Console.WriteLine("登录成功！");
            Console.WriteLine();

            var (needValidate, error) = await CheckRealnameStatusAsync();

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
            }

            if (!needValidate)
            {
                Console.ReadLine();
                return;
            }

            int total = _idCards.Count;
            bool authSuccess = false;
            var usedIndices = new HashSet<int>();

            for (int i = 0; i < _idCards.Count; i++)
            {
                int randomIndex;
                do
                {
                    randomIndex = _random.Next(0, _idCards.Count);
                } while (usedIndices.Contains(randomIndex) && usedIndices.Count < _idCards.Count);

                usedIndices.Add(randomIndex);
                
                var (name, idcard) = _idCards[randomIndex];
                Console.WriteLine($"尝试 [{i + 1}/{total}] 使用实名: {name}");

                var (ok, code, resultMsg) = await DoRealnameAsync(name, idcard);

                if (ok)
                {
                    Console.WriteLine($"实名认证成功！");
                    authSuccess = true;
                    break;
                }
                else
                {
                    Console.WriteLine($"未知错误实名结果: {resultMsg} ({code})");
                }
                
                await Task.Delay(500);
            }

            if (!authSuccess)
            {
                Console.WriteLine($"\n在 {total} 次尝试后实名认证失败。");
            }
        }
        
        public async Task RunFromFileAsync(string username, string password)
        {
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("4399 实名认证工具 (C# 版)");
            Console.WriteLine("原作者: freecookie.studio");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine();

            if (_idCards.Count == 0)
            {
                Console.WriteLine("错误: 没有有效的身份证信息。");
                return;
            }

            Console.WriteLine();

            var (success, msg) = await LoginAsync(username, password);
            if (!success)
            {
                Console.WriteLine(msg);
                return;
            }

            Console.WriteLine("登录成功！");
            Console.WriteLine();

            var (needValidate, error) = await CheckRealnameStatusAsync();

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
            }

            if (!needValidate)
            {
                Console.WriteLine("实名认证已完成 (之前已验证)！");
                return;
            }

            int total = _idCards.Count;
            bool authSuccess = false;
            var usedIndices = new HashSet<int>();

            for (int i = 0; i < _idCards.Count; i++)
            {
                int randomIndex;
                do
                {
                    randomIndex = _random.Next(0, _idCards.Count);
                } while (usedIndices.Contains(randomIndex) && usedIndices.Count < _idCards.Count);

                usedIndices.Add(randomIndex);
                
                var (name, idcard) = _idCards[randomIndex];
                Console.WriteLine($"尝试 [{i + 1}/{total}] 使用实名: {name}");

                var (ok, code, resultMsg) = await DoRealnameAsync(name, idcard);

                if (ok)
                {
                    Console.WriteLine($"实名认证成功！");
                    authSuccess = true;
                    break;
                }
                else
                {
                    Console.WriteLine($"未知错误实名结果: {resultMsg} ({code})");
                    
                    if (code == 3006)
                    {
                        Console.WriteLine("检测到登录状态异常，跳过当前账号，继续下一个...");
                        if (i < _idCards.Count - 1)
                        {
                            continue;
                        }
                    }
                }
                
                await Task.Delay(1000);
            }

            if (!authSuccess)
            {
                Console.WriteLine($"\n在 {total} 次尝试后实名认证失败。");
            }
        }
    }
}
