<?php
	require_once('db/db_exe.php');

	if (isset($_REQUEST['sql'])) {
		echo DBExecute::Query($_REQUEST['sql']);
	}

?>
