namespace Logic.Context
{
    public enum Response
    {
        Created = 200,
        Updated = 200,
        Deleted = 200,
        Forbidden = 403,
        NotFound = 404,
        BadRequest = 400,
        Conflict = 409,
    }
}