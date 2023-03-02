namespace IdentityService.Errors
{
    public class UserAlreadyExists : HttpException
    {
        public UserAlreadyExists() : base(400, ErrorType.AlreadyExsists, "A user with the provided email address already exists. Registration terminaded.")
        {
        }
    }
}
