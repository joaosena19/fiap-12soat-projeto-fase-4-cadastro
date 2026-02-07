output "db_instance_id" {
  description = "ID da instância PostgreSQL RDS do Cadastros"
  value       = aws_db_instance.cadastro_postgres.id
}

output "db_instance_endpoint" {
  description = "Endpoint da instância PostgreSQL RDS do Cadastros"
  value       = aws_db_instance.cadastro_postgres.endpoint
}

output "db_instance_address" {
  description = "Endereço (host) da instância PostgreSQL RDS do Cadastros"
  value       = aws_db_instance.cadastro_postgres.address
}

output "db_instance_port" {
  description = "Porta da instância PostgreSQL RDS do Cadastros"
  value       = aws_db_instance.cadastro_postgres.port
}

output "db_name" {
  description = "Nome do banco de dados do Cadastros"
  value       = aws_db_instance.cadastro_postgres.db_name
}

output "db_username" {
  description = "Username master do banco de dados do Cadastros"
  value       = aws_db_instance.cadastro_postgres.username
  sensitive   = true
}

output "db_security_group_id" {
  description = "ID do Security Group do PostgreSQL do Cadastros"
  value       = aws_security_group.cadastro_db_sg.id
}

output "db_subnet_group_name" {
  description = "Nome do DB Subnet Group do Cadastros"
  value       = aws_db_subnet_group.cadastro_subnet_group.name
}

# Connection string para uso nos ConfigMaps/Secrets do K8s
output "db_connection_string" {
  description = "Connection string do banco de dados do Cadastros (sem senha)"
  value       = "Host=${aws_db_instance.cadastro_postgres.address};Port=${aws_db_instance.cadastro_postgres.port};Database=${aws_db_instance.cadastro_postgres.db_name};Username=${aws_db_instance.cadastro_postgres.username}"
  sensitive   = true
}
