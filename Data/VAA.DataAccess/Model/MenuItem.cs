namespace VAA.DataAccess.Model
{
    public class MenuItem
    {
        public long Id { get; set; }        
        //menu
        public long MenuId { get; set; }
        public string MenuName { get; set; }

        //menu type
        public int MenuTypeId { get; set; }
        public string MenuTypeName { get; set; }
        
        //class
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        //route
        public long RouteId { get; set; }
        public string RouteName { get; set; }

        //category
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }

        //item details
        public long BaseItemId { get; set; }        
        public string BaseItemCode { get; set; }
        public string BaseItemTitle { get; set; }
        public string BaseItemTitleDescription { get; set; }
        public string BaseItemDescription { get; set; }
        public string BaseItemSubDescription { get; set; }
        public string BaseItemAttributes { get; set; }

        public int Sequence { get; set; }
    }
}
