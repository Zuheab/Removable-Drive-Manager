using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive_Manager
{
    class Stack
    {
        private List<String> locations { get; }
        private int top = 0;
        
        public Stack()
        {
            locations = new List<String>(); 
        }

        public void Push(String x)
        {
                top += 1;
                locations.Add(x);
        }

        public String Pop()
        {
            String x = "";
            try {
                top -= 1;
                x = locations[top];
                //locations.RemoveAt(top);
                
            } catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message +"  ----   "+top +"  ----"+locations.Count());
            }
            
            
            return x;
        }
        
        public String Top()
        {
            String x = "";
            try
            {
                x = locations[top];
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + "  ----   " + top + "  ----" + locations.Count());
            }
            return x;
        }

        public bool IsEmpty()
        {
            return top == -1;
        }

        public bool hasJustOne()
        {
            return top == 0;
        }
    }
}
