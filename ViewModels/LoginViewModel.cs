namespace DotNetCoreBoilerPlate.ViewModels
{
    public partial class LoginViewModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
    }

    public partial class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
