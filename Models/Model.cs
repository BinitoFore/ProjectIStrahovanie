using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurseProject.Models
{

    public class User : IdentityUser
    {

        public string Name { get; set; }
        public string SecName { get; set; }
        public string Pathonymic { get; set; }

        public Legal_entity legal_entity { get; set; }

        public string Passp_number { get; set; }


    }

    public class Legal_entity
    {
        [Key]
        [ForeignKey("user")]
        public string Id { get; set; }
        public string Leg_adress { get; set; }
        public string Paym_account { get; set; }
        public string Org_name { get; set; }

        public User user { get; set; }
    }

    public class Property
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string descript { get; set; }
        public User owner { get; set; }
        public List<Contract>? contracts { get; set; }

    }

    public class Contract
    {
        [Key]
        public int num_of_contr { get; set; }
        public User client { get; set; }
        public User agent { get; set; }
        public InsAmenities insAmenities { get; set; }
        public Risk risk { get; set; }
        public List<Property>? properties { get; set; }
        public float Ins_premium { get; set; }
        public float Ins_sum { get; set; }
        public float arrears { get; set; }
        public DateTime Start_date { get; set; }
        public DateTime End_date { get; set; }
        public bool Is_problem { get; set; }

    }

    public class InsAmenities
    {
        [Key]   
            
        public int Id { get; set; }
        public string Name { get; set; }
        public string descript { get; set; }
    }

    public class Risk
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string descrip { get; set; }
    }

    public class Req_for_paym
    {
        [Key]
        public int Id { get; set; }

        public string description { get; set; }

        public Contract contract { get; set; }

     }



}

