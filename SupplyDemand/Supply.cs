using System.Collections.Generic;

namespace SupplyDemand
{
    public class Supply
    {
        public int Id { get; set; }

        public int Capacity { get; set; }

        public int Categories { get; set; }

        public void SetCategories(params int[] presets)
        {
            foreach (int i in presets)
            {
                if (i <= 32)
                {
                    this.Categories = this.Categories | (1 << i);
                }
            }
        }

        public bool IsCategoryMatched(int demandCategory)
        {
            return (this.Categories == 0) || ((this.Categories >> demandCategory) & 1) == 1;
        }
    }
}
