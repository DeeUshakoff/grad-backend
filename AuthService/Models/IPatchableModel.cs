namespace AuthService.Models
{
    public interface IPatchableModel<T>
    {
        public void Patch(T patchDTO);
    }
}
