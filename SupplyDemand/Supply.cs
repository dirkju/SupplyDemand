using System.Collections.Generic;

namespace SupplyDemandSolver
{
    /// <summary>
    /// The class modelling a supply.
    /// </summary>
    public class Supply
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the constraint categories.
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public int Categories { get; set; }

        /// <summary>
        /// Sets the categories from int array of category numbers.
        /// </summary>
        /// <param name="categoryNumbers">The category numbers.</param>
        public void SetCategories(params int[] categoryNumbers)
        {
            foreach (int i in categoryNumbers)
            {
                if (i <= 32)
                {
                    this.Categories = this.Categories | (1 << i);
                }
            }
        }

        /// <summary>
        /// Determines whether this supply matches the specified demand category.
        /// </summary>
        /// <param name="demandCategory">The demand category.</param>
        /// <returns>True if matched, false otherwise.</returns>
        public bool IsCategoryMatched(int demandCategory)
        {
            return (this.Categories == 0) || ((this.Categories >> demandCategory) & 1) == 1;
        }
    }
}
