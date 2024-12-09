public interface IScroll
{
    public void DiscreteScroll(int delta);
    public void Scroll(float delta);
    public void ScrollToElement(int index);
    public int CurrentElement();
}
