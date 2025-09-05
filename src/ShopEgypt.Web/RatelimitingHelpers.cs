namespace WearUp.Web;
public static class RatelimitingHelpers
{
    public static string GetPartitionKey(HttpContext context,string formKey)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        string value = "anonymous";

        if (context.Request.HasFormContentType && context.Request.Form.ContainsKey(formKey))
        {
            value = context.Request.Form[formKey].ToString();
        }

        return $"{ip}:{value}";
    }

    public static string? GetPolicyName(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (path != null)
        {
            if (path.StartsWith("/auths/login") && context.Request.Method == "POST")
                return "login";

            if (path.StartsWith("/auths/register") && context.Request.Method == "POST")
                return "register";

            if (path.StartsWith("/auths/forgotpassword") && context.Request.Method == "POST")
                return "forgotPassword";
        }

        return null;
    }
}
