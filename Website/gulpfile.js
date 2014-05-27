var gulp = require('gulp');
var less = require('gulp-less')
var tinylr = require('tiny-lr');
var express = require('express');
var path = require('path');

var paths = {
  html: ['src/**/*.html'],
  icons: ['src/*.ico'],
  script: ['src/**/*.js', 'bower_components/angular/angular.js', 'bower_components/angular-route/angular-route.js'],
  css: ['bower_components/bootstrap/dist/css/*'],
  fonts: ['bower_components/bootstrap/dist/fonts/*'],
  images: ['src/images/*'],
  less: ['src/less/*.less']
};

gulp.task('images', function() {
  return gulp.src(paths.images)
    .pipe(gulp.dest('debug'));
});

gulp.task('icons', function() {
  return gulp.src(paths.icons)
    .pipe(gulp.dest('debug'));
});

gulp.task('vendor_script', function() {
  gulp.src(paths.script, { base: 'bower_components/' })
    .pipe(gulp.dest('debug/vendor'));
});

gulp.task('script', function() {
  return gulp.src(paths.script, { base: 'src/' })
    .pipe(gulp.dest('debug'));
});

gulp.task('css', function() {
  return gulp.src(paths.css)
    .pipe(gulp.dest('debug/css'));
});

gulp.task('fonts', function() {
  return gulp.src(paths.fonts)
    .pipe(gulp.dest('debug/fonts'));
});

gulp.task('html', function() {
  return gulp.src(paths.html, { base: 'src/' })
    .pipe(gulp.dest('debug'));
});

gulp.task('less', function() {
  return gulp.src(paths.less)
    .pipe(less())
    .pipe(gulp.dest('debug/css'));
});

gulp.task('watch', function() {
  startExpress();
  startLiveReload();
});

function startExpress() {
  source = express()
  source.use(require('connect-livereload')())
  source.use(express.static('debug'));
  source.listen(4000);
}

function startLiveReload() {
  lr = tinylr();
  lr.listen(35729);
  gulp.watch(paths.fonts, ['fonts']);
  gulp.watch(paths.html, ['html']);
  gulp.watch(paths.script, ['script']);
  gulp.watch(paths.images, ['images']);
  gulp.watch(paths.icons, ['icons']);
  gulp.watch(paths.css, ['css']);
  gulp.watch(paths.less, ['less']);
  gulp.watch(['debug/*', 'debug/**/*'], notifyLiveReload);
}

function notifyLiveReload(event) {
  // `gulp.watch()` events provide an absolute path
  // so we need to make it relative to the server root
  var fileName = path.relative("debug", event.path);
 
  lr.changed({
    body: {
      files: [fileName]
    }
  });
}

gulp.task('default', ['images', 'icons', 'html', 'script', 'vendor_script', 'css', 'less', 'fonts']);