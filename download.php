<?php
if(!empty($_GET['version']) && preg_match('#[0-9]+\.[0-9]+#', $_GET['version']))
{
	$globalVersion = 'v' . substr($_GET['version'], 0, 3);
	$path = 'downloads/' . $globalVersion . '/DnK-v' . $_GET['version'] . '.zip';
	if(file_exists($path))
	{
		header('Content-Description: File Transfer');
		header('Content-Type: application/octet-stream');
		header('Content-Disposition: attachment; filename=' . basename($path));
		header('Content-Transfer-Encoding: binary');
		header('Expires: 0');
		header('Cache-Control: must-revalidate, post-check=0, pre-check=0');
		header('Pragma: public');
		header('Content-Length: ' . filesize($path));
	
		// Increment counter
		$fp = fopen('hit.txt', 'r+'); 
		$hit = fgets($fp, 255); 
		$hit++; 
		fseek($fp, 0); 
		fputs($fp, $hit); 
		fclose($fp); 
	
		readfile($path);
	}
	else
	{
		header('Location: erreur.html');
	}
}
else
{
	header('Location: erreur.html');
}
?>