using System.Linq;
using System.Collections.Generic;
using MediaControlsLibrary.Models;

namespace MediaControlsLibrary.Extensions
{
    public static class StackExtension
    {
        public static Stack<Folder> Copy(this Stack<Stack<Folder>> stack)
        {
            //Initialize Variables
            Stack<Folder> stacky = new Stack<Folder>();

            //Get Last Element Added to Stack and Convert it to a List
            List<Folder> elements = stack.Peek().ToList();

            //Loop through Elements in elements List
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                //Add Current Looped Element to Stacky Stack
                stacky.Push(elements[i]);
            }

            //Return Stacky Stack
            return stacky;
        }
    }
}