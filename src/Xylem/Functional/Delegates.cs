
namespace Xylem.Functional
{
    public delegate void Receiver<V>(V v);
    public delegate void DualReceiver<X, V>(X x, V v);
    public delegate void Update();

    public delegate V Provider<V>();

    public delegate void Looper<V>(int i, V v);

    public delegate V Function<X, V>(X x);
    public delegate V DualFunction<X, Y, V>(X x, Y y);
}