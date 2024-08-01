public class PersonAnimation
{
    public ImageSeries imageSeries;
    public Person.BehaviorType behavior;
    public Person.MovementType movement;

    public PersonAnimation(ImageSeries imageSeries, Person.BehaviorType behavior, Person.MovementType movement)
    {
        this.imageSeries = imageSeries;
        this.behavior = behavior;
        this.movement = movement;
    }
}