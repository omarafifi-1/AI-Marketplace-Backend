using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.Products.DTOs
{
    public class EditProductDto
    {
        [Required(ErrorMessage = "Product Name is Required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name Must be Between 3 and 200 Characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description Cannot Exceed 2000 Characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is Required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price Must be Greater Than 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock Cannot be Negative")]
        public int Stock { get; set; } = 0;

        [Required(ErrorMessage = "Category is Required")]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; }

    }
}
