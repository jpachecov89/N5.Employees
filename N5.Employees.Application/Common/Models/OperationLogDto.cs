namespace N5.Employees.Application.Common.Models
{
	public class OperationLogDto
	{
		public Guid Id { get; set; }
		public string Operation { get; set; } = null!;
		public string Payload { get; set; } = null!;
	}
}