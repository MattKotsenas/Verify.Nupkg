# Contributing

// TODO

## How to release

1. Run `dotnet nbgv tag`
2. `git push origin {tag name}`
3. Navigate to GitHub releases page and create release from tag
4. (If desired) increment version (major or minor) in `//version.json > version`
5. `git add . && git commit -m 'Bumping version' && git push`
