namespace S5_01_App_CS_GOAT.Mapper;

public interface IMapper<Entity, DTO>
{
    Entity ToEntity(DTO dto);
    DTO ToDTO(Entity entity);

    IEnumerable<Entity> ToEntity(IEnumerable<DTO> entities)
    {
        return entities.Select(e => ToEntity(e));
    }

    IEnumerable<DTO> ToDTO(IEnumerable<Entity> entities)
    {
        return entities.Select(e => ToDTO(e));
    }
}
