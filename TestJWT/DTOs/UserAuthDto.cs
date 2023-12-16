namespace TestJWT.DTOs
{
	public class UserAuthDto
	{
		public string Message { get; set; }=string.Empty; 
		public bool IsAuthenticated { get; set; }
		public string Email { get; set;} = string.Empty;
		public string UserName { get; set;} = string.Empty;
		public List<string> Roles { get; set; } = new List<string>();
		public string Token { get; set; } = string.Empty;
		public DateTime ExpiresOn { get; set; }
	}
}
