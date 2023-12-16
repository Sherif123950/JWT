namespace TestJWT.Helpers
{
	public class JWT
	{
        public string SecurityKey { get; set; }=string.Empty;
		public string Issuer { get; set; } = string.Empty;
		public string Audience { get; set; } = string.Empty;
		public double DurationInDays { get; set; } 
	}
}
