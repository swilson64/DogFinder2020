//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DogFinder
{
    using System;
    using System.Collections.Generic;
    
    public partial class Dog
    {
        public long DogID { get; set; }
        public string OwnerID { get; set; }
        public int TypeID { get; set; }
        public int Age { get; set; }
        public bool Sex { get; set; }
        public string Name { get; set; }
        public int StatusID { get; set; }
        public string Description { get; set; }
        public string FavFood { get; set; }
        public int SizeID { get; set; }
        public string Allergies { get; set; }
        public bool IsDangerous { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual DogStatu DogStatu { get; set; }
        public virtual DogSize DogSize { get; set; }
        public virtual DogType DogType { get; set; }
    }
}
