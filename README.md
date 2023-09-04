# DiscordNetTemplate
Just a bot template I usually use for my bots.
It's built using `HostBuilder` & `InteractionService`, which allows easy modification & good maintainability of the codebase.

Feel free to use it as a base for your own projects. 

### Usage
- Clone the repo or create new repository from this template
- (Optional) Rename `DiscordNetTemplate.sln`, `DiscordNetTemplate` folder, `DiscordNetTemplate/DiscordNetTemplate.csproj` to whatever name you want for the project. Then open `DiscordNetTemplate.sln` in text editor & change all `DiscordNetTemplate` references to your bot name. Then edit all namespaces to be also your bot name.
- Replace `token` in `appsettings.json` with your actual bot token, which can be obtained from you application's page on [dev portal](https://discord.com/developers/applications)
- Compile & run the app. If you did everything correctly - congrats!!  your bot is online!
- By default bot comes with a single slash command - `/test`. The bot should register the command when it comes online. Run the command to test if it actually works - you should get `Hello There` in the response.
