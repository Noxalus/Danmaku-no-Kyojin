<!doctype html>
<html>
  <head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="chrome=1">
    <title>Danmaku no Kyojin - Contact</title>
    <link rel="stylesheet" href="stylesheets/styles.css">
    <link rel="stylesheet" href="stylesheets/pygment_trac.css">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
    <script src="javascripts/respond.js"></script>
    <!--[if lt IE 9]>
      <script src="//html5shiv.googlecode.com/svn/trunk/html5.js"></script>
    <![endif]-->
    <!--[if lt IE 8]>
    <link rel="stylesheet" href="stylesheets/ie.css">
    <![endif]-->
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=no">

  </head>
  <body>
      <div id="header">
        <nav>
          <li class="fork"><a href="https://github.com/Noxalus/Danmaku-no-Kyojin">Source code</a></li>
          <li class="downloads"><a href="https://github.com/Noxalus/Danmaku-no-Kyojin/zipball/master">Download the demo</a></li>
        </nav>
      </div>

    <div class="wrapper">

      <section>
        <div id="title">
          <h1>Contact</h1>
          <hr>
        </div>

				<form action="contact.php" method="POST">
					<fieldset>
						<p>
						<label for="email">E-mail:<br /></label>
						<input type="text" name="email" placeholder="johndoe@gmail.com" id="email" />
						</p>
						
						<p>
						<label for="message">Message:<br /></label>
						<textarea name="message" placeholder="Please enter your message here." id="message" cols="60" rows="10"></textarea>
						</p>
						
						<p>
						<input type="submit" value="Send" />
						</p>
					</fieldset>
				</form>
				<p>
					<a href="index.php" title="Back to home">Back to home</a>
				</p>
      </section>

    </div>
    <!--[if !IE]><script>fixScale(document);</script><![endif]-->
    
  </body>
</html>