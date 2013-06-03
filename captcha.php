<?php
session_start();
 
if(!isset($_GET['random_number']))
     $random_number = 6;
else
     $random_number = $_GET['random_number'];


header ("Content-type: image/png");

$image = imagecreatefrompng('images/captcha-bg.png');


$background = imagecolorallocate($image, 0, 0, 0);
$foreground = imagecolorallocate($image, 255, 255, 255);


$i = 0;
while($i < $random_number) {
        $digit = mt_rand(0, 9);
        $digits[$i] = $digit;
        $i++;
}
$number = null;

foreach ($digits as $character) {
        $number .= $character;
}

$_SESSION['random_number'] = $number;

unset($digit);
unset($i);
unset($character);
unset($digits);

imagestring($image, 5, 18, 8, $number, $foreground);

imagepng($image);