
using EastSide.Core.Api;
using Serilog;

namespace EastSide.Core.Utils;

public static class CaptchaRecognitionService
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    public static async Task<string?> RecognizeFromUrlAsync(string captchaUrl)
    {
        try
        {
            Log.Debug("[CaptchaRecognition] 正在下载验证码图片: {Url}", captchaUrl);
            var imageBytes = await _httpClient.GetByteArrayAsync(captchaUrl);
            var base64 = Convert.ToBase64String(imageBytes);
            return await RecognizeFromBase64Async(base64);
        }
        catch (Exception ex)
        {
            Log.Warning("[CaptchaRecognition] 从URL识别验证码失败: {Error}", ex.Message);
            return null;
        }
    }

    public static async Task<string?> RecognizeFromBase64Async(string base64)
    {
        try
        {
            Log.Debug("[CaptchaRecognition] 正在调用验证码识别 API");
            var result = await OxygenApi.Instance.RecognizeCaptchaAsync(base64);
            if (string.IsNullOrWhiteSpace(result))
            {
                Log.Warning("[CaptchaRecognition] API 返回空结果");
                return null;
            }
            Log.Information("[CaptchaRecognition] 验证码识别成功: {Result}", result);
            return result;
        }
        catch (TaskCanceledException)
        {
            Log.Warning("[CaptchaRecognition] 验证码识别请求超时");
            return null;
        }
        catch (HttpRequestException ex)
        {
            Log.Warning("[CaptchaRecognition] 无法连接到验证码识别服务器: {Error}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Log.Warning("[CaptchaRecognition] 验证码识别失败: {Error}", ex.Message);
            return null;
        }
    }
}
