var gulp = require('gulp'),
  argv = require('yargs').argv,
  less = require('gulp-less')
  tinylr = require('tiny-lr'),
  express = require('express'),
  path = require('path'),
  mainBowerFiles = require('main-bower-files'),
  rimraf = require('gulp-rimraf'),
  ngConstant = require('gulp-ng-constant'),
  rename = require('gulp-rename'),
  markdown = require('gulp-markdown'),
  spawn = require('gulp-spawn');

var expPort = 4000;
var lrPort = 35729;
var appPath = 'app/';
var docPath = 'doc/';
var buildPath = 'build/';
var bowerComponentsPath = 'bower_components/';

var paths = {
  config: [argv.release ? 'config.release.json' : 'config.debug.json'],
  html: ['**/*.html', '!tson_format.html'],
  icons: ['*.ico'],
  script: ['**/*.js'],
  images: ['**/*.png'],
  less: ['**/*.less'],
  markdown: ['**/*.md']
};

for (var name in paths) {
  paths[name] = paths[name].map(function(value) {
    return path.join(appPath, value);
  });
}

gulp.task('clean', function() {
  return gulp.src(buildPath, {read: false})
    .pipe(rimraf());
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
  return gulp.src(mainBowerFiles({checkExistence: true}), { base: bowerComponentsPath })
    .pipe(gulp.dest(path.join(buildPath, 'lib')));
});

gulp.task('markdown', function() {
  return gulp.src(paths.markdown)
    .pipe(markdown())
    .pipe(gulp.dest(path.join(buildPath))); 
})

gulp.task('serve', ['default'], function() {
  startExpress();
  startLiveReload();
});

function startExpress() {
  app = express()
  app.use(require('connect-livereload')({
    port: lrPort
  }));
  app.use(express.static(buildPath));
  // NOTE: We could use connect-modrewrite here, but this seems sufficient
  app.use(redirectToHashPath);
  app.listen(expPort);
  console.log('express listening on %s', expPort);
}

function redirectToHashPath(req, res) {
  if (res.req.path.indexOf('/#') != 0) {
    res.redirect("/#" + req.path);
  }
}

function startLiveReload() {
  lr = tinylr();
  lr.listen(lrPort, function() {
    console.log('tiny-lr listening on %s', lrPort);
  });
  gulp.watch(paths.images, ['images']);
  gulp.watch(paths.icons, ['icons']);
  gulp.watch(paths.html, ['html']);
  gulp.watch(paths.script, ['script']);
  gulp.watch(paths.less, ['less']);
  gulp.watch(paths.config, ['config']);
  gulp.watch(paths.markdown, ['markdown']);
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

gulp.task('watchdot', ['dot'], function() {
  gulp.watch(path.join(docPath, "*.dot"), ['dot']);
  console.log('Watching for ' + docPath + ' changes');
});

gulp.task('dot', function() {
  return gulp.src(path.join(docPath, '*.dot'))
    .pipe(spawn({
      cmd: 'dot', 
      args: ['-Tsvg'],
      filename: function(base, ext) {
        return base + '.svg'
      }
    }))
    .pipe(gulp.dest(docPath))
});

gulp.task('default', ['images', 'icons', 'html', 'script', 'less', 'config', 'lib', 'markdown']);
