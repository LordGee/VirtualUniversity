<?php

	require_once('db_conn.php');

	class DBExecute extends DBConnection {
		
		protected $pdo;

		public function __construct() {
			parent::__construct();
			$this->pdo = $this->GetPdo();
		}

		public function Query($sql) {
			$statement = $this->pdo->prepare($sql);
			$statement->execute();
			if (substr( $sql, 0, 6 ) === "SELECT") {
				$result = $statement->fetchAll(PDO::FETCH_ASSOC);
				return json_encode($result);
			} 
			return 1;
		}
	}

	$DB = new DBExecute();
?>