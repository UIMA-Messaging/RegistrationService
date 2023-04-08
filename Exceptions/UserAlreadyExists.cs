namespace RegistrationApi.Exceptions
{
    public class UserAlreadyExists : HttpException
    {
        public UserAlreadyExists() : base(400, "A user with the provided display name already exists. Registration terminaded.")
        {
        }
    }
}
