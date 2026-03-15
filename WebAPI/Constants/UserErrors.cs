using CamCon.Shared;

namespace WebAPI.Constants
{
    public static class UserErrors
    {
        public static Error UserExist(string id) => new (StatusCodes.Status400BadRequest, $"User with email {id} already exists");
        public static Error FailedToCreateUserRoles(string roleErrors) => new(StatusCodes.Status500InternalServerError, $"Failed to create user role. Errors : {roleErrors}");
        public static Error FailedToCreateUser(string errors) => new(StatusCodes.Status500InternalServerError, $"Failed to create user. Errors: {errors}");
        public static Error Unauthorized() => new(StatusCodes.Status401Unauthorized, "Unauthorized access");
        public static Error EmailNotConfirmed() => new(StatusCodes.Status403Forbidden, "email_not_confirmed");
    }
}
