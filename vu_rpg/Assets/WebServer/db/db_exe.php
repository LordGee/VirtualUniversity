<?php

	require_once('db_conn.php');

	class DBExecute extends DBConnection {
		
		protected $pdo;
		private static $init = false;

		public function __construct() {
			parent::__construct();
			$this->pdo = $this->GetPdo();
		}

		private static function Initialize() {
			if (self::$init) { return; }

			parent::__construct();
			$this->pdo = $this->GetPdo();
			self::$init = true;
		}

		public static function Query($sql) {
			self::Initialize();
			$statement = $this->pdo->prepare($sql);
			$statement->execute();
			$result = $statement->fetchAll(PDO::FETCH_ASSOC);
			return json_encode($result);
		}

?>