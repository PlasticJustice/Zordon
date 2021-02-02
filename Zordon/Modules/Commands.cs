using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Zordon.Modules {
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class ExampleCommands : ModuleBase {
        [Command("help")]
        public async Task HelpMsg([Remainder] string cmd) {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
            if (cmd == "role-rename") {
                embed.Title = "role-rename Help!";
                sb.AppendLine(".role-rename [Role's Current Name] [Role's New Name]");
                sb.AppendLine("");
                sb.AppendLine("if [Role's Current Name] contains quotation marks, then you need to use a backslash to escape.");
                sb.AppendLine(" Ex: I'm \"Serious\" => I'm \\\\\"Serious\\\\\"");
                sb.AppendLine("");
                sb.AppendLine("if [Role's Current Name] is more than one word or contains spaces, it must be surrounded by quotation marks.");
                sb.AppendLine(" Ex: I'm a Bot => \"I'm a Bot\"");
            } else if (cmd == "role-colour" || cmd == "role-color") {
                embed.Title = "role-colour Help!";
                sb.AppendLine(".role-colour [Role's Name] [Hex Colour Code]");
                sb.AppendLine("");
                sb.AppendLine("if [Role's Name] contains quotation marks, then you need to use a backslash to escape.");
                sb.AppendLine(" Ex: I'm \"Serious\" => I'm \\\\\"Serious\\\\\"");
                sb.AppendLine("");
                sb.AppendLine("if [Role's Name] is more than one word or contains spaces, it must be surrounded by quotation marks.");
                sb.AppendLine(" Ex: I'm a Bot => \"I'm a Bot\"");
                sb.AppendLine("");
                sb.AppendLine("[Hex Colour Code] is something like this: #F7A8B8");
            } else {
                embed.Title = "Error";
                sb.AppendLine("Not a vaild command or there is no help info.");
            }
            embed.Description = sb.ToString();
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("dog")]
        public async Task Dog() {
            var sb = new StringBuilder();
            sb.AppendLine("Somebody deserved this");
            sb.AppendLine("https://cdn.discordapp.com/attachments/434468293211062294/804915049830678528/image0.gif");
            await ReplyAsync(sb.ToString());
        }

        [Command("sleep")]
        public async Task Sleep() {
            var sb = new StringBuilder();
            sb.AppendLine("https://cdn.discordapp.com/attachments/523358648022597652/658475677934157874/BedOclock.jpg");
            await ReplyAsync(sb.ToString());
        }

        [Command("shrug")]
        public async Task Shrug() {
            var sb = new StringBuilder();
            sb.AppendLine(@"¯\_(ツ)_/¯");
            await ReplyAsync(sb.ToString());
        }

        [Command("tableflip")]
        public async Task Tableflip() {
            var sb = new StringBuilder();
            Random gen = new Random();
            byte i = (byte)gen.Next(1, 4);
            switch (i) {
                case 1:
                    sb.AppendLine(@"(╯°□°）╯︵ ┻━┻");
                    break;
                case 2:
                    sb.AppendLine(@"(ノಠ益ಠ)ノ彡┻━┻");
                    break;
                case 3:
                    sb.AppendLine(@"┻━┻︵ヽ(`Д´)ﾉ︵ ┻━┻");
                    break;
            }           
            await ReplyAsync(sb.ToString());
        }

        [Command("unflip")]
        public async Task Unflip() {
            var sb = new StringBuilder();
            Random gen = new Random();
            byte i = (byte)gen.Next(1, 3);
            switch (i) {
                case 1:
                    sb.AppendLine(@"┬─┬ ノ( ゜-゜ノ)");
                    break;
                case 2:
                    sb.AppendLine(@"┬──┬ ¯\_(ツ)");
                    break;
            }
            await ReplyAsync(sb.ToString());
        }

        [Command("transflag")]
        public async Task TransFlag() {
            var sb = new StringBuilder();
            sb.AppendLine(@"🏳️‍⚧️");
            await ReplyAsync(sb.ToString());
        }

        [Command("yay")]
        public async Task Yay() {
            var sb = new StringBuilder();
            sb.AppendLine("https://cdn.discordapp.com/attachments/434468293211062294/805262011272396800/yay.gif");
            await ReplyAsync(sb.ToString());
        }

        [Command("role-rename")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RoleRename(SocketRole role, [Remainder] string newname) {
            var sb = new StringBuilder();
            var oldname = role.Name;
            UInt64[] people = {390549124581294082, 390549525133000704, 390549799876952074};
            if (role.Members.Contains(Context.Message.Author) || !people.Contains(role.Id)) { 
            var rolypoly = role.Guild.GetRole(role.Id);
            await rolypoly.ModifyAsync(x => {
                x.Name = newname;
            });
            sb.AppendLine($"[{oldname}] renamed to [{newname}]!");
            } else {
                sb.AppendLine($"You are not a member of [{oldname}]!");
            }
            
            await ReplyAsync(sb.ToString());
        }

        [Command("role-colour")]
        [Alias("role-color")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RoleColour(SocketRole role, [Remainder] string colour) {
            var sb = new StringBuilder();
            UInt64[] people = {390549124581294082, 390549525133000704, 390549799876952074};
            if (role.Members.Contains(Context.Message.Author) || !people.Contains(role.Id)) {
                var rolypoly = role.Guild.GetRole(role.Id);
                uint argb = UInt32.Parse(colour.Replace("#", ""), System.Globalization.NumberStyles.HexNumber);
                await rolypoly.ModifyAsync(x => {
                    x.Color = new Color(argb);
                });
                sb.AppendLine($"[{role.Name}] coloured to [{colour}]!");
            } else {
                sb.AppendLine($"You are not a member of [{role.Name}]!");
            }
            await ReplyAsync(sb.ToString());
        }

        [Command("role-info")]
        public async Task RoleInfo(SocketRole role) {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
            embed.Title = role.Name;
            embed.Color = role.Color;
            sb.AppendLine($"Colour: {role.Color}");
            string mem = "";
            foreach (SocketGuildUser m in role.Members) {
                //mem += m.Username + "#" + m.Discriminator + (m.Nickname != null ? " (" + m.Nickname + ")" : "") + "\n";
                mem += (m.Nickname != null ? m.Nickname + " (" + m.Username + "#" + m.Discriminator + ")" : m.Username + "#" + m.Discriminator) + "\n";
            }            
            sb.AppendLine("Members:\n" + $"{mem}".PadLeft(6));
            sb.AppendLine($"ID: {role.Id}");
            sb.AppendLine($"Created: {role.CreatedAt.LocalDateTime}");
            embed.Description = sb.ToString();
            await ReplyAsync(null, false, embed.Build());
        }

            [Command("hello")]
        public async Task HelloCommand() {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            // get user info from the Context
            var user = Context.User;

            // build out the reply
            sb.AppendLine($"You are -> [{Context.User.Username}]");
            sb.AppendLine("I must now say, World!");

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        [Command("8ball")]
        [Alias("ask")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task AskEightBall([Remainder] string args = null) {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            // now to create a list of possible replies
            var replies = new List<string>();

            // add our possible replies
            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("hazzzzy....");

            // time to add some options to the embed (like color and title)
            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Welcome to the 8-ball!";

            // we can get lots of information from the Context that is passed into the commands
            // here I'm setting up the preface with the user's name and a comma
            sb.AppendLine($",");
            sb.AppendLine();

            // let's make sure the supplied question isn't null 
            if (args == null) {
                // if no question is asked (args are null), reply with the below text
                sb.AppendLine("Sorry, can't answer a question you didn't ask!");
            }
            else {
                // if we have a question, let's give an answer!
                // get a random number to index our list with (arrays start at zero so we subtract 1 from the count)
                var answer = replies[new Random().Next(replies.Count - 1)];

                // build out our reply with the handy StringBuilder
                sb.AppendLine($"You asked: [****]...");
                sb.AppendLine();
                sb.AppendLine($"...your answer is [****]");

                // bonus - let's switch out the reply and change the color based on it
                switch (answer) {
                    case "yes": {
                            embed.WithColor(new Color(0, 255, 0));
                            break;
                        }
                    case "no": {
                            embed.WithColor(new Color(255, 0, 0));
                            break;
                        }
                    case "maybe": {
                            embed.WithColor(new Color(255, 255, 0));
                            break;
                        }
                    case "hazzzzy....": {
                            embed.WithColor(new Color(255, 0, 255));
                            break;
                        }
                }
            }

            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }
    }
}