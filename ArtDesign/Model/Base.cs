namespace ArtDesign.Model
{
    /// <summary>
    /// Базовый класс, от которого наследуются все сущности.
    /// </summary>
    public abstract class Base
    {
        public virtual int ID { get; set; }
    }
}