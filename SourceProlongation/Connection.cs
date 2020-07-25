﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceProlongation
{
    public static class Connection
    {
        public static SqlConnection Con { get; private set; }

        public static string ConnectionString { get; private set; }

        public static bool IsSuccessful { get; private set; }

        public static void Initialize(string conString)
        {
            if (conString == null)
            {
                throw new Exception("Connection string is null");
            }
            if (conString.Length < 1)
            {
                throw new Exception("Connection string length < 0");
            }

            IsSuccessful = true;
            try
            {
                Con = new SqlConnection(conString);
                ConnectionString = conString;
                Con.Open();
            }
            catch (Exception ex)
            {
                IsSuccessful = false;
            }
            finally
            {
                Con.Close();
            }
        }


        private static void Open()
        {
            if (Con.State != ConnectionState.Open) Con.Open();
        }

        private static void Close()
        {
            if (Con.State != ConnectionState.Closed) Con.Close();
        }

        public static object ExecuteScalar(SqlCommand command)
        {
            Open();
            var res = command.ExecuteScalar();
            Close();
            return res;
        }
    }
}
