using System;
using System.Linq;
using TreasureMap;

namespace TresorMap
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Map map = new Map();
                map.Init(FileHelper.ReadFiles());
                while(map.PlayTurn())
                {

                }

                FileHelper.SaveFile(map.SaveState());
            }
            catch (ArgumentException argEx)
            {

                FileHelper.Log($"Error during initialization of the game : {argEx.Message}");
            }
            catch(Exception ex)
            {
                FileHelper.Log($"An error occurred : {ex.Message}");
            }

        }
    }
}
