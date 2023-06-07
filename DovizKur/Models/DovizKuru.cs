using System;

namespace DovizKur.Models
{
    public class DovizKuru
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public decimal EuroKuru { get; set; }
        public decimal DolarKuru { get; set; }
    }
}
