using System;

namespace VAA.DataAccess.Model
{
    public class BaseItem
    {
        public long BaseItemId { get; set; }
        public string BaseItemCode { get; set; }
        public long ? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string BaseItemTitle { get; set; }
        public string BaseItemTitleDescription { get; set; }
        public string BaseItemDescription { get; set; }
        public string BaseItemSubDescription { get; set; }
        public string BaseItemAttributes { get; set; }
        public int ? ClassId { get; set; }
        public int ? MenuTypeId { get; set; }
        public int? LanguageId { get; set; }
    }
}
