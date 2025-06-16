Sub Main()
    Dim connStr As String = "Data Source=.;Initial Catalog=YOUR_DB;Integrated Security=True"
    Dim helper As New StoredProcedureHelper(connStr)

    Using conn As New SqlConnection(connStr)
        conn.Open()
        Using tran As SqlTransaction = conn.BeginTransaction()
            Try
                ' เรียก SaveTransSummary
                helper.SaveTransSummary("20250615", 123, "01", 100D, 10, 5, 2, 1, 3, 2, 1, 500D, 1, 24, tran)

                ' เรียก InsertTbTransMT
                helper.InsertTbTransMT(1001, "MT001", "20250615", 123, 10, 100D, 1000D, tran)

                ' เรียก InsertTbTransYO
                helper.InsertTbTransYO("YO001", "CUS001", "SHIP001", "20250615", 123, 5, 200D, 1000D, tran)

                ' เรียก InsertTbTransCJ
                helper.InsertTbTransCJ("CJ001", "CUS002", "SHIP002", "20250615", 123, 3, 300D, 900D, tran)

                ' Commit
                tran.Commit()
                Console.WriteLine("✅ บันทึกข้อมูลเรียบร้อยแล้ว")

            Catch ex As Exception
                tran.Rollback()
                Console.WriteLine("❌ เกิดข้อผิดพลาด: " & ex.Message)
            End Try
        End Using
    End Using
End Sub
