using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyDemand
{
    public class ConstraintCategories
    {
        public const int MAXCATEGORIES = 32;
        private BitArray categories;

        public ConstraintCategories()
        {
            categories = new BitArray(MAXCATEGORIES);
        }

        public ConstraintCategories(params int[] presets) : this()
        {
            foreach(int i in presets)
            {
                if(i <= MAXCATEGORIES)
                {
                    categories.Set(i, true);
                }
            }
        }

        public void Set(int position, bool value)
        {
            categories.Set(position, value);
        }

        public bool Get(int position)
        {
            return categories.Get(position);
        }

        public bool Match(int position)
        {
            return categories.Get(position);
        }
    }
}
