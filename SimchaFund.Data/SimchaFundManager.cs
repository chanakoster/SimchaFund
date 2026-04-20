using Microsoft.Data.SqlClient;

namespace SimchaFund.Data
{
    public class SimchaFundManager
    {
        private readonly string _connectionString;

        public SimchaFundManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddSimcha(Simcha simcha)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Simchas (Name, Date) VALUES (@name, @date) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", simcha.Name);
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            conn.Open();

            return (int)(decimal)cmd.ExecuteScalar();

        }

        public int AddContributor(Contributor contributor)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributors (FirstName, LastName, CellNumber, Date, AlwaysInclude) VALUES (@firstName, @lastName, @cellNumber, @date, @alwaysInclude) " +
                "SELECT SCOPE_IDENTITY();";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@date", contributor.Date);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public void UpdateContributor(Contributor contributor)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Contributors SET FirstName = @firstName, LastName = @lastName, CellNumber = @cellNumber, AlwaysInclude = @alwaysInclude WHERE Id = @id";
            cmd.Parameters.AddWithValue("@firstName", contributor.FirstName);
            cmd.Parameters.AddWithValue("@lastName", contributor.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            cmd.Parameters.AddWithValue("@id", contributor.Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddDeposit(int contributorId, Transaction transaction)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Deposits (ContributorId, Amount, Date) VALUES (@contributorId, @amount, @date)";
            cmd.Parameters.AddWithValue("@contributorId", contributorId);
            cmd.Parameters.AddWithValue("@amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@date", transaction.Date);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddContribution(Contribution contribution)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributions (ContributorId, SimchaId, Amount) VALUES (@contributorId, @simchaId, @amount)";
            cmd.Parameters.AddWithValue("@contributorId", contribution.ContributorId);
            cmd.Parameters.AddWithValue("@simchaId", contribution.SimchaId);
            cmd.Parameters.AddWithValue("@amount", contribution.Amount);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteContributionsBySimcha(int simchaId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Contributions " +
                "WHERE SimchaId = @simchaId";
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddContributions(List<Contribution> contributions)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributions (ContributorId, SimchaId, Amount) VALUES (@contributorId, @simchaId, @amount)";
            conn.Open();
            foreach (var contribution in contributions.Where(c => c.Included))
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@contributorId", contribution.ContributorId);
                cmd.Parameters.AddWithValue("@simchaId", contribution.SimchaId);
                cmd.Parameters.AddWithValue("@amount", contribution.Amount);
                cmd.ExecuteNonQuery();
            }
        }

        //public void UpdateContributions(List<Contribution> contributions)
        //{
        //    using var conn = new SqlConnection(_connectionString);
        //    using var cmd = conn.CreateCommand();
        //    //cmd.CommandText = "INSERT INTO Contributions (ContributorId, SimchaId, Amount) VALUES (@contributorId, @simchaId, @amount)";
        //    cmd.CommandText = @"
        //UPDATE Contributions
        //SET Amount = @amount
        //WHERE ContributorId = @contributorId
        //AND SimchaId = @simchaId";
        //    conn.Open();
        //    foreach (var contribution in contributions)
        //    {
        //        cmd.Parameters.Clear();
        //        cmd.Parameters.AddWithValue("@contributorId", contribution.ContributorId);
        //        cmd.Parameters.AddWithValue("@simchaId", contribution.SimchaId);
        //        cmd.Parameters.AddWithValue("@amount", contribution.Amount);
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        public List<Simcha> GetSimchas()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT s.Id, s.Name, con.Id as ContributorId, c.Amount, s.Date FROM Simchas s " +
                "LEFT JOIN Contributions c on c.SimchaId = s.Id " +
                "LEFT JOIN Contributors con on c.ContributorId = con.Id";
            //cmd.CommandText = "SELECT s.Id, s.Name, con.Id as ContributorId, c.Amount, s.Date FROM Simchas s " +
            //                  "LEFT JOIN Contributors con on c.ContributorId = con.Id " +
            //                  "LEFT JOIN Contributions c on c.SimchaId = s.Id";
            conn.Open();

            List<Simcha> simchas = new List<Simcha>();
            var reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                int simchaId = (int)reader["Id"];
                Simcha simcha = simchas.FirstOrDefault(s => s.Id == simchaId);
                if (simcha == null)
                {
                    {
                        simcha = new Simcha
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Date = (DateTime)reader["Date"],
                        };
                        simchas.Add(simcha);
                    }
                }

                int? contributorId = reader.GetOrNull<int?>("ContributorId");

                //if (contributorId.HasValue && !simcha.Contributors.Contains(contributorId.Value))
                //{
                //    simcha.Contributors.Add(contributorId.Value);

                //    decimal amount = (decimal)reader["Amount"];

                //    simcha.Contributions.Add(amount);
                //}

                decimal amount = reader.GetOrNull<decimal>("Amount");
                if (contributorId.HasValue && amount > 0 && !simcha.Contributors.Contains(contributorId.Value))
                {
                    simcha.Contributors.Add(contributorId.Value);

                    simcha.Contributions.Add(amount);
                }

            }

            return simchas;
        }

        public Simcha GetSimcha(int simchaId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Select * From Simchas WHERE ID = @id";
            cmd.Parameters.AddWithValue("@id", simchaId);
            conn.Open();
            var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new Simcha
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Date = (DateTime)reader["Date"],
            };

        }

        public int GetTotalContributors()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public List<Contributor> GetContributors()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors";
            conn.Open();
            var reader = cmd.ExecuteReader();

            List<Contributor> contributors = new List<Contributor>();

            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Date = (DateTime)reader["Date"]
                });
            }

            return contributors;
        }

        public Contributor GetContributor(int contributorId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors WHERE Id = @contributorId";
            cmd.Parameters.AddWithValue("@contributorId", contributorId);
            conn.Open();
            var reader = cmd.ExecuteReader();

            List<Contributor> contributors = new List<Contributor>();

            if (!reader.Read())
            {
                return null;
            }

            return new Contributor
            {
                Id = (int)reader["Id"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                CellNumber = (string)reader["CellNumber"],
                AlwaysInclude = (bool)reader["AlwaysInclude"]
            };
        }

        public decimal GetContributorBalance(int contributorId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT
(SELECT ISNULL (SUM(Amount), 0) FROM Deposits WHERE ContributorId = @contributorId) AS DepositTotal,
(SELECT ISNULL (SUM(Amount), 0) FROM Contributions WHERE ContributorId = @contributorId) AS ContributionTotal";
            cmd.Parameters.AddWithValue("@contributorId", contributorId);

            conn.Open();
            var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return 0;
            }
            return (decimal)reader["DepositTotal"] - (decimal)reader["ContributionTotal"];
        }

        public List<Deposit> GetDepositsByContributor(int contributorId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Deposits 
                                WHERE ContributorId  = @contributorID";
            cmd.Parameters.AddWithValue("@contributorId", contributorId);
            conn.Open();
            var reader = cmd.ExecuteReader();

            List<Deposit> deposits = new List<Deposit>();

            while (reader.Read())
            {
                deposits.Add(new Deposit
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    TransactionMemo = "Deposit"
                });
            }

            return deposits;
        }

        public List<Contribution> GetContributionsByContributor(int contributorId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM  Contributions c
                                JOIN Simchas s ON s.Id = c.SimchaId
                                WHERE ContributorId  = @contributorID";
            cmd.Parameters.AddWithValue("@contributorId", contributorId);
            conn.Open();
            var reader = cmd.ExecuteReader();

            List<Contribution> contributions = new List<Contribution>();

            while (reader.Read())
            {
                contributions.Add(new Contribution
                {
                    Amount = -1 * ((decimal)reader["Amount"]),
                    Date = (DateTime)reader["Date"],
                    TransactionMemo = "Contribution for the " + (string)reader["Name"] + " Simcha"
                });
            }

            return contributions;
        }

        public List<Contribution> GetContributions(int simchaId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributions WHERE SimchaId = @simchaId";
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            conn.Open();
            var reader = cmd.ExecuteReader();

            List<Contribution> contributions = new List<Contribution>();

            while (reader.Read())
            {
                contributions.Add(new Contribution
                {
                    SimchaId = (int)reader["SimchaId"],
                    ContributorId = (int)reader["ContributorId"],
                    Amount = (decimal)reader["Amount"],
                });
            }

            return contributions;
        }

        //public void DeleteContribution(int contributionId)
        //{
        //    using var conn = new SqlConnection(_connectionString);
        //    using var cmd = conn.CreateCommand();
        //    cmd.CommandText = "DELETE FROM Contributions WHERE Id =  "
        //}


        public void AddDeposit(Deposit deposit)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = " INSERT INTO Deposits (ContributorId, Amount, Date) " +
                "VALUES (@contributorId, @amount, @Date)";
            cmd.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
            cmd.Parameters.AddWithValue("@amount", deposit.Amount);
            cmd.Parameters.AddWithValue("@date", deposit.Date);

            conn.Open();
            cmd.ExecuteNonQuery();
        }


    }
}
