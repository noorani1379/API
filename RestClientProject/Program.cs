
using RestSharp;
using RestSharp.Authenticators;

Console.WriteLine("Hello RestSharp!");

        var client = new RestClient("https://localhost:44379/");
        //get  sms Code
        var getSmsRequest = new RestRequest("/api/Accounts/GetSmsCode", Method.Get);
        getSmsRequest.AddParameter("PhoneNumber", "09128698172");
        var getSmsResult = client.Get(getSmsRequest);

        //send smsCode and Get Token

        ;
        var getTokenRequest = new RestRequest("/api/Accounts", Method.Post);
        getTokenRequest.AddJsonBody(new { PhoneNumber = "09111111111", SmsCode = "1957" });
        //var getTokenResult= client.Post(getTokenRequest);
        var getTokenResult = client.Post<LoginResultDto>(getTokenRequest);
       

        
        

        
        Console.ReadLine();





public class LoginResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public LoginDataDto Data { get; set; }
}

public class LoginDataDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}