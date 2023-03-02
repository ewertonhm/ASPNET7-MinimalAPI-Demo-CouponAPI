namespace CouponAPI.Models.Dto
{
    public class CouponCreateDTO
    {
        public string Name { get; set; }
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}
