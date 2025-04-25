namespace AspNetTutorial.Dtos;


// public class BaseParams {
// 	public string? ApiKey { get; set; }
// }

public class LoginParams {
	public required string Email { get; set; }
	public required string Password { get; set; }
	
}

public class LoginResponse {
	public required string Token { get; set; }
}