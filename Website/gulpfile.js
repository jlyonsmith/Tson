var gulp = require('gulp'),
  argv = require('yargs').argv,
  less = require('gulp-less')
  tinylr = require('tiny-lr'),
  express = require('express'),
  path = require('path'),
  gulpBowerFiles = require('gulp-bower-files'),
  clean = require('gulp-clean'),
  ngConstant = require('gulp-ng-constant'),
  rename = require('gulp-rename');

var portNum = 4000;
var appPath = 'app/';
var buildPath = 'build/';

var paths = {
  config: [argv.release ? 'config.release.json' : 'config.debug.json'],
  html: ['**/*.html', '!tson_format.html'],
  icons: ['*.ico'],
  script: ['**/*.js'],
  images: ['**/*.png'],
  less: ['**/*.less']
};

for (var name in paths) {
  paths[name] = paths[name].map(function(value) {
    return path.join(appPath, value);
  });
}

gulp.task('clean', function() {
  return gulp.src(buildPath, {read: false})
    .pipe(clean());
});

gulp.task('images', function() {
  return gulp.src(paths.images, { base: appPath })
    .pipe(gulp.dest(buildPath));
});

gulp.task('icons', function() {
  return gulp.src(paths.icons)
    .pipe(gulp.dest(buildPath));
});

gulp.task('script', function() {
  return gulp.src(paths.script, { base: appPath })
    .pipe(gulp.dest(buildPath));
});

gulp.task('html', function() {
  return gulp.src(paths.html, { base: appPath })
    .pipe(gulp.dest(buildPath));
});

gulp.task('less', function() {
  return gulp.src(paths.less, { base: appPath })
    .pipe(less())
    .pipe(gulp.dest(buildPath));
});

gulp.task('config', function() {
  return gulp.src(paths.config, { base: appPath })
    .pipe(ngConstant())
    .pipe(rename({basename: 'config'}))
    .pipe(gulp.dest(buildPath));
});

gulp.task('lib', function() {
  var src = gulpBowerFiles({checkExistence: true})
    .pipe(gulp.dest(path.join(buildPath, "lib")));
});

gulp.task('serve', function() {
  startExpress();
  startLiveReload();
  console.log('Running on port', portNum);
});

function startExpress() {
  source = express()
  source.use(require('connect-livereload')())
  source.use(express.static(buildPath));
  source.listen(portNum);
}

function startLiveReload() {
  lr = tinylr();
  lr.listen(35729);
  gulp.watch(paths.html, ['html']);
  gulp.watch(paths.script, ['script']);
  gulp.watch(paths.images, ['images']);
  gulp.watch(paths.icons, ['icons']);
  gulp.watch(paths.less, ['less']);
  gulp.watch(paths.config, ['config']);
  gulp.watch([path.join(buildPath, '*'), path.join(buildPath, '**/*')], notifyLiveReload);
}

function notifyLiveReload(event) {
  // `gulp.watch()` events provide an absolute path
  // so we need to make it relative to the server root
  var fileName = path.relative(buildPath, event.path);
 
  lr.changed({
    body: {
      files: [fileName]
    }
  });
}

gulp.task('default', ['images', 'icons', 'html', 'script', 'less', 'config', 'lib']);
