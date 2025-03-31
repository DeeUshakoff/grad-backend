namespace AuthService.DTO.Project
{
    public class PatchProjectDTO : IPatchableDTO<Models.Project.Project>
    {
        public string? Name { get; set; }
        public int? ActivityStatus { get; set; }
        public string? Description { get; set; }

        public void ApplyEntity(Models.Project.Project entity)
        {
            Name = entity.Name;
            ActivityStatus = (int)entity.ActivityStatus;
            Description = entity.Description;
        }
    }
}