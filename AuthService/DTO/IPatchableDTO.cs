namespace AuthService.DTO
{
    public interface IPatchableDTO<T>
    {
        public void ApplyEntity(T entity);
    }
}
