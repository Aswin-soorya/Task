using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using FormCreation.IRepository;
using FormCreation.Models;

namespace FormCreation.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _conn =
            ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        public List<DropdownItem> GetCountries()
        {
            var list = new List<DropdownItem>();
            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand("SELECT * FROM Country", con);
                con.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    list.Add(new DropdownItem
                    {
                        Id = Convert.ToInt32(dr["CountryId"]),
                        Name = dr["CountryName"].ToString()
                    });
            }
            return list;
        }

        public List<DropdownItem> GetStatesByCountry(int countryId)
        {
            var list = new List<DropdownItem>();
            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand(
                    "SELECT * FROM State WHERE CountryId = @Id", con);
                cmd.Parameters.AddWithValue("@Id", countryId);
                con.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    list.Add(new DropdownItem
                    {
                        Id = Convert.ToInt32(dr["StateId"]),
                        Name = dr["StateName"].ToString()
                    });
            }
            return list;
        }

        public List<DropdownItem> GetDistrictsByState(int stateId)
        {
            var list = new List<DropdownItem>();
            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand(
                    "SELECT * FROM District WHERE StateId = @Id", con);
                cmd.Parameters.AddWithValue("@Id", stateId);
                con.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    list.Add(new DropdownItem
                    {
                        Id = Convert.ToInt32(dr["DistrictId"]),
                        Name = dr["DistrictName"].ToString()
                    });
            }
            return list;
        }
        public void SaveUser(UserDetails u)
        {
            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand(@"
        INSERT INTO UserDetails(UserName,CountryId,StateId,DistrictId)
        OUTPUT INSERTED.UserId
        VALUES(@n,@c,@s,@d)", con);

                cmd.Parameters.AddWithValue("@n", u.UserName);
                cmd.Parameters.AddWithValue("@c", u.CountryId);
                cmd.Parameters.AddWithValue("@s", u.StateId);
                cmd.Parameters.AddWithValue("@d", u.DistrictId);

                con.Open();
                int newId = (int)cmd.ExecuteScalar();

                InsertAudit(new AuditLogModel
                {
                    TableName = "UserDetails",
                    ActionType = "INSERT",
                    RecordId = newId,
                    NewData = $"Name:{u.UserName},Country:{u.CountryId},State:{u.StateId},District:{u.DistrictId}",
                    DoneBy = "Admin"
                });
            }
        }

        public List<UserDetails> GetUsers()
        {
            var list = new List<UserDetails>();

            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand(@"
        SELECT 
            u.UserId,
            u.UserName,
            ISNULL(c.CountryName,'') AS CountryName,
            ISNULL(s.StateName,'') AS StateName,
            ISNULL(d.DistrictName,'') AS DistrictName
        FROM UserDetails u
        LEFT JOIN Country c ON u.CountryId = c.CountryId
        LEFT JOIN State s ON u.StateId = s.StateId
        LEFT JOIN District d ON u.DistrictId = d.DistrictId
        ", con);

                con.Open();
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new UserDetails
                    {
                        UserId = Convert.ToInt32(dr["UserId"]),
                        UserName = dr["UserName"].ToString(),
                        CountryName = dr["CountryName"].ToString(),
                        StateName = dr["StateName"].ToString(),
                        DistrictName = dr["DistrictName"].ToString()
                    });
                }
            }

            return list;
        }
        public UserDetails GetUserById(int id)
        {
            UserDetails u = new UserDetails();

            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand("SELECT * FROM UserDetails WHERE UserId=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    u.UserId = (int)dr["UserId"];
                    u.UserName = dr["UserName"].ToString();
                    u.CountryId = (int)dr["CountryId"];
                    u.StateId = (int)dr["StateId"];
                    u.DistrictId = (int)dr["DistrictId"];
                }
            }
            return u;
        }

        public void UpdateUser(UserDetails u)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();

                var oldCmd = new SqlCommand("SELECT * FROM UserDetails WHERE UserId=@id", con);
                oldCmd.Parameters.AddWithValue("@id", u.UserId);
                var dr = oldCmd.ExecuteReader();

                string oldData = "";
                if (dr.Read())
                {
                    oldData = $"Name:{dr["UserName"]},Country:{dr["CountryId"]},State:{dr["StateId"]},District:{dr["DistrictId"]}";
                }
                dr.Close();

                var cmd = new SqlCommand(@"
        UPDATE UserDetails 
        SET UserName=@n,CountryId=@c,StateId=@s,DistrictId=@d,UpdatedDate = GETDATE() 
        WHERE UserId=@id", con);

                cmd.Parameters.AddWithValue("@id", u.UserId);
                cmd.Parameters.AddWithValue("@n", u.UserName);
                cmd.Parameters.AddWithValue("@c", u.CountryId);
                cmd.Parameters.AddWithValue("@s", u.StateId);
                cmd.Parameters.AddWithValue("@d", u.DistrictId);

                cmd.ExecuteNonQuery();

                string newData = $"Name:{u.UserName},Country:{u.CountryId},State:{u.StateId},District:{u.DistrictId}";

                InsertAudit(new AuditLogModel
                {
                    TableName = "UserDetails",
                    ActionType = "UPDATE",
                    RecordId = u.UserId,
                    OldData = oldData,
                    NewData = newData,
                    DoneBy = "Admin"
                });
            }
        }
        public void DeleteUser(int id)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();

                var oldCmd = new SqlCommand("SELECT * FROM UserDetails WHERE UserId=@id", con);
                oldCmd.Parameters.AddWithValue("@id", id);
                var dr = oldCmd.ExecuteReader();

                string oldData = "";
                if (dr.Read())
                {
                    oldData = $"Name:{dr["UserName"]},Country:{dr["CountryId"]},State:{dr["StateId"]},District:{dr["DistrictId"]}";
                }
                dr.Close();

                var cmd = new SqlCommand("DELETE FROM UserDetails WHERE UserId=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                InsertAudit(new AuditLogModel
                {
                    TableName = "UserDetails",
                    ActionType = "DELETE",
                    RecordId = id,
                    OldData = oldData,
                    DoneBy = "Admin"
                });
            }
        }
        public void InsertAudit(AuditLogModel log)
        {
            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand(@"
        INSERT INTO AuditLog(TableName,ActionType,RecordId,OldData,NewData,DoneBy)
        VALUES(@t,@a,@rid,@old,@new,@by)", con);

                cmd.Parameters.AddWithValue("@t", log.TableName);
                cmd.Parameters.AddWithValue("@a", log.ActionType);
                cmd.Parameters.AddWithValue("@rid", log.RecordId);
                cmd.Parameters.AddWithValue("@old", (object)log.OldData ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@new", (object)log.NewData ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@by", log.DoneBy ?? "Admin");

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<AuditLogModel> GetAuditLogs()
        {
            var list = new List<AuditLogModel>();

            using (var con = new SqlConnection(_conn))
            {
                var cmd = new SqlCommand("SELECT * FROM AuditLog ORDER BY AuditId DESC", con);
                con.Open();
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new AuditLogModel
                    {
                        TableName = dr["TableName"].ToString(),
                        ActionType = dr["ActionType"].ToString(),
                        RecordId = Convert.ToInt32(dr["RecordId"]),
                        OldData = dr["OldData"]?.ToString(),
                        NewData = dr["NewData"]?.ToString(),
                        DoneBy = dr["DoneBy"].ToString(),
                        DoneDate = Convert.ToDateTime(dr["DoneDate"]) 
                    });
                }
            }

            return list;
        }
    }
}