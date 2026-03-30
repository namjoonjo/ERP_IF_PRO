using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMBINATION.Modules
{
    class ProductionRecords
    {
        private string production_date { get; set; }

        private string lot_number { get; set; }

        private string item_code { get; set; }

        private string item_name { get; set; }

        private string good_quantity { get; set; }

        public ProductionRecords(string production_date, string lot_number, string item_code, string item_name, string good_quantity)
        {
            this.production_date = production_date;
            this.lot_number = lot_number;
            this.item_code = item_code;
            this.item_name = item_name;
            this.good_quantity = good_quantity;
        }

        public string GetProduction_date() {  return production_date; }

        public string GetIot_number() { return lot_number; }

        public string Getitem_code() { return item_code; }

        public string Getitem_name() {  return item_name; }

        public string Getgood_quantity() { return good_quantity; }
    }
}
