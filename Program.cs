using KirosEngine3.Math.Vector;
using KirosEngine3.testclients;

namespace KirosEngine3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                int screenWidth = int.Parse(args[0]);
                int screenHeight = int.Parse(args[1]);

                using Client client = new TestClient(screenWidth, screenHeight);
                client.Run();
            }
            else 
            {
                using Client client = new TestClient(800, 600);
                client.Run();
            }
        }
    }
}
