ui            = true
cluster_addr  = "http://0.0.0.0:8201"
api_addr      = "http://0.0.0.0:8200"
disable_mlock = true

storage "postgresql" {
  connection_url = "postgresql://vault:strongpassword@postgres-vault-service:5432/vault",
  "table": "vault_kv_store",
  "ha_enabled": true,
  "ha_table": "vault_ha_locks"
}

listener "tcp" {
  address       = "0.0.0.0:8200"
  tls_disable = 1
}