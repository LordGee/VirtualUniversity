<?php
	ini_set('display_errors', 1);
	ini_set('display_startup_errors', 1);
	error_reporting(E_ALL);
    
    class DBConnection {
        private $db_host = "localhost";
        private $db_name = "mychaosc_uni_rpg";
        private $db_user = "mychaosc_unirpg";
        private $db_pass = "yvJB2z8IUZQD";
        protected $pdoConn;
        public function __construct() {
            try {
                $this->pdoConn = new PDO("mysql:host=$this->db_host;dbname=$this->db_name;charset=utf8mb4", $this->db_user, $this->db_pass);
                $this->pdoConn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
            } catch (PDOException $ex) {
                echo 'CONNECTION ERROR: ' . $ex->getMessage();
            }
        }
        public function GetPdo() {
            return $this->pdoConn;
        }
    }
?>