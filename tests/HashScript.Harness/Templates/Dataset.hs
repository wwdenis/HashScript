namespace #Namespace#.#Dataset#
{
    using System;
    using System.Data;
    using #RootNamespace#;

    public class #Model#Row : DbRow
    {
        internal #Model#Row(DataRowBuilder builder) : base(builder)
        {
        }
#+Properties#
        public #Type# #Name#
        {
            get => this.Get<#Type#>();
            set => this.Set(value);
        }
#+#
#+Properties#
        public bool Is#Name#Null() => this.HasNull();
#+#
#+Properties#
        public void Set#Name#Null() => this.SetNull();
#+#
    }
}